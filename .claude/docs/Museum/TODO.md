# Museum Night Shift — TODO

Fonte: `.claude/docs/Multiplayer Chaos Game Ideas.pdf` — ideia #4 "Museum Night Shift".

## Premissa

Jogadores transportam artefatos valiosos para fora do museu antes da abertura. Os artefatos são
assombrados: estátuas observam os jogadores, pinturas mudam o layout das salas, esqueletos de
dinossauro desabam, máscaras antigas possuem jogadores, portas somem, salas giram.

Escopo desta primeira fatia (poucos dias, experimentação com agents): provar o *core loop*
multiplayer (carregar/entregar artefato) + 1–2 sistemas de caos, não o jogo completo.

## Convenções do projeto (seguir, não reinventar)

- Owner-gating via `enabled = IsOwner;` em `OnStartClient()` (ver `MyPlayerController`).
- `SyncVar<T>` genérico (FishNet 4.x), `OnChange` assinado em `Awake()`, cancelado em `OnDestroy()`.
- Fluxo de RPC: input do cliente → `[ServerRpc]` → (se outros clientes precisam observar) → `[ObserversRpc]`.
- Scripts novos em `Assets/_Project/Scripts/FishNetTest/` (ou nova subpasta `Museum/` dentro dela —
  decidir ao começar a implementação, seguindo o namespace `_Project` já usado por
  `MyPlayerIdProvider`/`MyPlayerColorChanger`).
- Prefabs novos em `Assets/_Project/Prefabs/`.
- Não editar `Assets/FishNet/` (vendor).

## Fase 0 — Fundação de sessão (bloqueador de tudo)

- [ ] Confirmar/configurar `NetworkManager` na cena (transporte, spawn de jogadores) — hoje não há
      wiring de sessão descrito no código; checar a cena no Editor antes de assumir.
- [ ] Definir ponto de spawn dos jogadores dentro do museu.
- [ ] Verificar: host + 1 cliente conseguem conectar e ambos veem o `MyPlayer` um do outro.

## Fase 1 — Core loop: carregar e entregar artefato

- [ ] `MuseumArtifact` (`NetworkObject` + `NetworkBehaviour`): item carregável, com estado
      sincronizado (`SyncVar<bool> IsHeld`, `SyncVar<NetworkObject> Holder` ou similar).
- [ ] Interação de pegar/soltar: `[ServerRpc]` do jogador solicitando pegar o artefato mais próximo;
      servidor valida (ninguém mais segurando) e autoriza; anexa o artefato ao jogador
      (client-side ou via `NetworkObject` parenting/ownership — decidir qual API do FishNet usar).
- [ ] Zona de entrega (`DropOffZone`, trigger collider): ao detectar artefato + jogador na zona,
      servidor valida e marca artefato como "entregue" (`SyncVar<bool>`), soma pontuação da partida.
- [ ] SyncVar de pontuação/objetivo geral da partida (quantos artefatos entregues / total).
- [ ] Verificar: dois clientes conseguem pegar artefatos diferentes simultaneamente sem conflito;
      um jogador não pode roubar o artefato que outro já está segurando (validação no servidor).

## Fase 2 — Caos #1: Estátua que persegue (menor escopo, maior impacto visual)

- [ ] `HauntedStatue`: estátua com `NavMeshAgent` (ou movimento simples) que fica parada quando
      observada por um jogador (`OnStartClient`/tick no servidor calcula ângulo de visão de cada
      jogador vs. a estátua) e avança lentamente quando ninguém olha.
- [ ] Estado sincronizado via `SyncVar<bool> IsBeingWatched` (ou calculado direto no servidor sem
      precisar sync, se a lógica de movimento já roda só no servidor com `NetworkTransform`).
- [ ] Se a estátua alcança um jogador: efeito simples (ex.: derruba o artefato que ele carrega).
- [ ] Verificar: comportamento é determinado no servidor (autoridade), clientes apenas veem o
      resultado via `NetworkTransform`; não há flicker/pop de posição perceptível.

## Fase 3 — Caos #2: Modificador de sala rotativa (opcional, se sobrar tempo)

- [ ] Uma sala com plataforma/rotação periódica (`SyncVar` de ângulo ou `NetworkTransform` do
      objeto raiz da sala) que reorganiza o caminho até a saída.
- [ ] Verificar: rotação é suave e sincronizada entre clientes (sem desync perceptível).

## Fase 4 — Loop de partida mínimo

- [ ] Timer de partida (`SyncVar<float>` decrescente, atualizado só no servidor).
- [ ] Condição de vitória/derrota simples: todos os artefatos entregues antes do timer zerar.
- [ ] Tela/estado de fim de partida (pode ser só um log/UI mínima — não é o foco desta fatia).

## Fora de escopo (por ora)

- Múltiplas salas/mapas, mobília fugitiva, geladeiras, pianos, etc. (ideias de outros minigames do GDD).
- Modificadores aleatórios de partida (gravidade baixa, blackout, etc.) — ideia geral do GDD,
  não específica do museu; avaliar depois que o core loop estiver estável.
- Arte final/assets — usar placeholders (cubos/cápsulas) enquanto valida a mecânica.
- UI polida.

## Notas

- Testar sempre em host + pelo menos 1 client build/ParrelSync (ou 2 instâncias do Editor, se
  configurado) — bugs de autoridade client/servidor não aparecem rodando só como host solo.
- Manter os sistemas de caos como componentes independentes e plugáveis (cada um seu próprio
  `NetworkBehaviour`), para poder ligar/desligar e combinar depois — alinhado com o princípio do
  GDD de "construir sistemas, não conteúdo".
